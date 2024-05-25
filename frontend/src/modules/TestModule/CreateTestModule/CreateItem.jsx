import { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { generate as uniqueId } from 'shortid';

import { Button, Tag, Form, Divider } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import { ArrowLeftOutlined, ArrowRightOutlined, CloseCircleOutlined } from '@ant-design/icons';

import { settingsAction } from '@/redux/settings/actions';
import { erp } from '@/redux/erp/actions';
import { selectCreatedItem } from '@/redux/erp/selectors';
import { selectLangDirection } from '@/redux/translate/selectors';
import useLanguage from '@/locale/useLanguage';
import Loading from '@/components/Loading';

export default function CreateItem({ config, CreateForm }) {
  const translate = useLanguage();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const langDirection = useSelector(selectLangDirection)

  useEffect(() => {
    dispatch(settingsAction.list({ entity: 'setting' }));
  }, []);

  let { entity } = config;
  const { isLoading, isSuccess, result } = useSelector(selectCreatedItem);
  const [form] = Form.useForm();

  useEffect(() => {
    if (isSuccess) {
      form.resetFields();
      dispatch(erp.resetAction({ actionType: 'create' }));
      navigate(`/${entity.toLowerCase()}`);
    }
    return () => { };
  }, [isSuccess]);

  const [curTopicId, setCurTopicId] = useState('');
  const handleTopicChange = (e) => {
    setCurTopicId(e);
    form.setFieldsValue({ experiments: [] }); // Clear the experiments form list
  };

  const onSubmit = (fieldsValue) => {
    console.log('🚀 ~ onSubmit ~ fieldsValue:', fieldsValue);
    if (fieldsValue) {
      if (fieldsValue.items) {
        let newList = [...fieldsValue.items];
        fieldsValue = {
          ...fieldsValue,
          items: newList,
        };
      }
    }
    dispatch(erp.create({ entity, jsonData: fieldsValue }));
  };

  return (
    <>
      <PageHeader
        backIcon={langDirection === "rtl" ? <ArrowRightOutlined /> : <ArrowLeftOutlined />}
        title={translate('New Test')}
        ghost={false}
        tags={<Tag>{translate('Draft')}</Tag>}
        extra={[
          <Button
            key={`${uniqueId()}`}
            onClick={() => navigate(`/${entity.toLowerCase()}`)}
            icon={<CloseCircleOutlined />}
          >
            {translate('Cancel')}
          </Button>,
        ]}
        style={{
          padding: '20px 0px',
        }}
      ></PageHeader>
      <Divider dashed />
      <Loading isLoading={isLoading}>
        <Form form={form} layout="vertical" onFinish={onSubmit} initialValues={{ testCode: uniqueId().toUpperCase() }}>
          <CreateForm handleTopicChange={handleTopicChange} curTopicId={curTopicId} />
        </Form>
      </Loading>
    </>
  );
}
