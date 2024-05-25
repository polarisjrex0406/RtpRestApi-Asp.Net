import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { generate as uniqueId } from 'shortid';

import { Button, Row, Col, Descriptions, Tag, Divider } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import { EditOutlined, CloseCircleOutlined } from '@ant-design/icons';

import { erp } from '@/redux/erp/actions';
import { selectCurrentItem } from '@/redux/erp/selectors';
import useLanguage from '@/locale/useLanguage';

const TopicPromptItem = ({ prompts, currentErp }) => {
  const colorTags = ['magenta', 'green', 'red', 'cyan', 'volcano', 'blue', 'orange', 'greekblue', 'gold', 'purple', 'lime']
  return (
    <Row gutter={[12, 0]} key={`${uniqueId()}`}>
      <Col className="gutter-row" span={24} key={`${uniqueId()}`}>
        {(() => {
          const promptTags = [];
          for (const i in prompts) {
            const colorIndex = parseInt(i) % colorTags.length;
            promptTags.push(<Tag key={`${uniqueId()}`} color={colorTags[colorIndex]}>
              {prompts[i]}
            </Tag>);
          }
          return promptTags;
        })()}
      </Col>
      <Divider dashed style={{ marginTop: 0, marginBottom: 15 }} />
    </Row>
  );
};

const TemplateItem = ({ order, item, currentErp }) => {

  return (
    <Row gutter={[12, 0]} key={item._id}>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {order}
        </p>
      </Col>
      <Col className="gutter-row" span={21}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.templateCode.name}
        </p>
      </Col>
      <Divider dashed style={{ marginTop: 0, marginBottom: 15 }} />
    </Row>
  );
};

export default function ReadItem({ config, selectedItem }) {
  const translate = useLanguage();
  const { entity, ENTITY_NAME } = config;
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const { result: currentResult } = useSelector(selectCurrentItem);

  const resetErp = {
    removed: false,
    enabled: true,
    experimentCode: '',
    description: '',
    style: 'Stand-alone',
    topic: '',
    templates: [],
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

  return (
    <>
      <PageHeader
        onBack={() => {
          navigate(`/${entity.toLowerCase()}`);
        }}
        title={`${ENTITY_NAME}`}
        extra={[
          <Button
            key={`${uniqueId()}`}
            onClick={() => {
              navigate(`/${entity.toLowerCase()}`);
            }}
            icon={<CloseCircleOutlined />}
          >
            {translate('Close')}
          </Button>,
          <Button
            key={`${uniqueId()}`}
            onClick={() => {
              dispatch(
                erp.currentAction({
                  actionType: 'update',
                  data: currentErp,
                })
              );
              navigate(`/${entity.toLowerCase()}/update/${currentErp._id}`);
            }}
            type="primary"
            icon={<EditOutlined />}
          >
            {translate('Edit')}
          </Button>,
        ]}
        style={{
          padding: '20px 0px',
        }}
      >
      </PageHeader>
      <Divider dashed />
      <Descriptions title={`${currentErp?.experimentCode}`}>
        <Descriptions.Item label={translate('topic')}>{currentErp?.topic?.name}</Descriptions.Item>
        <Descriptions.Item label={translate('description')}>{currentErp?.description}</Descriptions.Item>
        <Descriptions.Item label={translate('style')}>{currentErp?.style}</Descriptions.Item>
      </Descriptions>
      <Divider />
      <Descriptions title='Init Prompts' />
      {currentErp ? <TopicPromptItem prompts={currentErp?.initPrompt} /> : ''}
      <Descriptions title='Artifacts' />
      <Row gutter={[12, 0]}>
        {/* Key, Value Type, Value, Description, Prompt Modifier */}
        <Col className="gutter-row" span={3}>
          <p
            style={{
              textAlign: 'left',
            }}
          >
            <strong>{translate('Order')}</strong>
          </p>
        </Col>
        <Col className="gutter-row" span={3}>
          <p
            style={{
              textAlign: 'left',
            }}
          >
            <strong>{translate('Artifact Name')}</strong>
          </p>
        </Col>
      </Row>
      {currentErp?.templates?.map((item, index) => (
        <TemplateItem key={'temp' + index} order={index + 1} item={item} currentErp={currentErp} />
      ))}
    </>
  );
}
