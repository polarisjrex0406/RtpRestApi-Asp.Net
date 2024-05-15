import { useState, useEffect } from 'react';
import { Divider } from 'antd';

import { Button, Row, Col, Descriptions, Statistic, Tag, Alert, Card, Collapse } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import {
  EditOutlined,
  FilePdfOutlined,
  CloseCircleOutlined,
  RetweetOutlined,
  MailOutlined,
  DeleteOutlined,
} from '@ant-design/icons';

import { useSelector, useDispatch } from 'react-redux';
import useLanguage from '@/locale/useLanguage';
import { erp } from '@/redux/erp/actions';

import { generate as uniqueId } from 'shortid';

import { selectCurrentItem } from '@/redux/erp/selectors';

import { DOWNLOAD_BASE_URL } from '@/config/serverApiConfig';
import useMail from '@/hooks/useMail';
import { useNavigate } from 'react-router-dom';
import { tagColor } from '@/utils/statusTagColor';
import { settingsAction } from '@/redux/settings/actions';

const SystemMessage = ({ role, content, order }) => {
  return (
    <>
      <Col key={'3Col' + order} className="gutter-row" span={3} />
      <Col key={'Col18' + order} className="gutter-row" span={18}>
        <Alert
          key={'Alert' + order}
          message="Initial Prompt"
          description={content}
          type="error"
        />
      </Col>
      <Col key={'Col3' + order} className="gutter-row" span={3} />
      <Divider />
    </>
  );
}

const UserMessage = ({ role, content, order }) => {
  return (
    <>
      <Col key={'Col6' + order} className="gutter-row" span={6} />
      <Col key={'Col18' + order} className="gutter-row" span={18}>
        <Alert
          key={'Alert' + order}
          message="Refined Prompt"
          description={content}
          type="info"
        />
      </Col>
      <Divider dashed />
    </>
  );
}

const AssistMessage = ({ role, content, order }) => {
  return (
    <>
      <Col key={'Col18' + order} className="gutter-row" span={18}>
        <Alert
          key={'Alert' + order}
          message="Response"
          description={content}
          type="success"
        />
      </Col>
      <Col key={'Col6' + order} className="gutter-row" span={6} />
      <Divider />
    </>
  );
}

const PromptItem = ({ role, content, order }) => {
  if (role === 'system') {
    return (
      <SystemMessage key={role + order} content={content} order={role + order} />
    );
  }
  else if (role === 'user') {
    return (
      <UserMessage key={role + order} content={content} order={role + order} />
    );
  }
  else if (role === 'assistant') {
    return (
      <AssistMessage key={role + order} content={content} order={role + order} />
    );
  }
  else {
    return;
  }
}

const ChatItem = ({ order, item }) => {
  return (
    <Row gutter={[12, 0]} key={'Row' + order}>
      {item?.input?.map((inp, index) => (
        <PromptItem key={'PromptItem' + index} role={inp?.role} content={inp?.content} order={index} />
      ))}
      <PromptItem key={'PromptItemOut'} role={item?.output?.role} content={item?.output?.content} order={'Out'} />
    </Row>
  );
}

const ExperimentItem = ({ item }) => {
  const artiRows = [];
  return <Collapse items={artiRows} defaultActiveKey={['1']} />;
};

export default function ReadItem({ config, selectedItem }) {
  const translate = useLanguage();
  const { entity, ENTITY_NAME } = config;
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const { send, isLoading: mailInProgress } = useMail({ entity });

  const { result: currentResult } = useSelector(selectCurrentItem);

  const resetErp = {
    removed: false,
    enabled: true,
    testCode: '',
    description: '',
    topic: {},
    experiments: [],
  };

  const [currentErp, setCurrentErp] = useState(selectedItem ?? resetErp);

  useEffect(() => {
    if (currentResult) {
      setCurrentErp(currentResult);
    }
    return () => {
      setCurrentErp(resetErp);
    };
  }, [currentResult]);

  useEffect(() => {
  }, [currentErp]);

  return (
    <>
      <PageHeader
        onBack={() => {
          window.history.back();
        }}
        title={`${ENTITY_NAME} - ${currentErp?.testCode} : ${currentErp?.topicName}`}
        extra={[
          <Button
            key={`${uniqueId()} `}
            onClick={() => {
              window.history.back();
            }}
            icon={<CloseCircleOutlined />}
          >
            {translate('Close')}
          </Button>,
        ]}
        style={{
          padding: '20px 0px',
        }}
      >
      </PageHeader>
      {(() => {
        const expRows = [];
        for (const i in currentErp?.experiments) {
          let key = '';
          key += i + 1;
          let label = '{' + currentErp?.topicPrompt?.join('}, ') + '}';
          label = label === '{}' ? '' : label + ' : ';
          label += currentErp?.experiments[i]?.experimentCode;
          expRows.push({
            key: key,
            label: label,
            children: <ExperimentItem item={currentErp?.experiments[i]?.artifacts} />
          });
        }

        return <Row gutter={[12, 12]}>
          <Col
            className="gutter-row"
            xs={{ span: 24 }}
            sm={{ span: 24 }}
            md={{ span: 24 }}
            lg={{ span: 24 }}
          >
            {
              (() => {
                if (expRows.length > 0) {
                  return <Collapse items={expRows} defaultActiveKey={['1']} />;
                }
              })()
            }
          </Col>
        </Row>;
      })()}
    </>
  );
}
