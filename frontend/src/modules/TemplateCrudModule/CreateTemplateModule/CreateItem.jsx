import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { generate as uniqueId } from 'shortid';

import { Button, Tag, Form, Divider } from 'antd';
import { ArrowLeftOutlined, ArrowRightOutlined, CloseCircleOutlined, PlusOutlined } from '@ant-design/icons';
import { PageHeader } from '@ant-design/pro-layout';

import { settingsAction } from '@/redux/settings/actions';
import { erp } from '@/redux/erp/actions';
import { selectCreatedItem } from '@/redux/erp/selectors';
import { selectLangDirection } from '@/redux/translate/selectors';

import useLanguage from '@/locale/useLanguage';
import Loading from '@/components/Loading';

function SaveForm({ form }) {
  const translate = useLanguage();
  const handelClick = () => {
    form.submit();
  };

  return (
    <Button onClick={handelClick} type="primary" icon={<PlusOutlined />}>
      {translate('Save')}
    </Button>
  );
}

export default function CreateItem({ config, CreateForm }) {
  const translate = useLanguage();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const pathSegments = window.location.pathname.split('/');
  const methodFromUrl = pathSegments[pathSegments.length - 2];
  const chatIdFromUrl = pathSegments[pathSegments.length - 1];

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
      navigate(`/template/${chatIdFromUrl}`);
    }
    return () => { };
  }, [isSuccess]);

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
  const langDirection = useSelector(selectLangDirection);

  return (
    <>
      <PageHeader
        backIcon={langDirection === "rtl" ? <ArrowRightOutlined /> : <ArrowLeftOutlined />}

        title={translate('New Artifact')}
        ghost={false}
        tags={<Tag>{translate('Draft')}</Tag>}
        extra={[
          <Button
            key={`${uniqueId()}`}
            onClick={() => navigate(`/template/${chatIdFromUrl}`)}
            icon={<CloseCircleOutlined />}
          >
            {translate('Cancel')}
          </Button>,
          <SaveForm form={form} key={`${uniqueId()}`} />,
        ]}
        style={{
          padding: '20px 0px',
        }}
      ></PageHeader>
      <Divider dashed />
      <Loading isLoading={isLoading}>
        <Form
          form={form}
          layout="vertical"
          onFinish={onSubmit}
          initialValues={{ cacheTimeoutValue: '0', topic: methodFromUrl === 'create' ? chatIdFromUrl : '' }}>
          <CreateForm />
        </Form>
      </Loading>
    </>
  );
}
